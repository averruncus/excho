namespace Excho.Logistics

type IInventory =
  abstract member Unload : obj * Volumn -> Package
  abstract member Load : Package -> unit
and IInventory<'t> =
  inherit IInventory
  abstract member Unload : 't * Volumn -> Package<'t>
  abstract member Load : Package<'t> -> unit
and Package = Source * Quantitizable
and Package<'t> = 't Source * 't Quantitizable
and Source = IInventory
and Source<'t> = 't IInventory
and Volumn = int
and Quantitizable = obj * Volumn
and Quantitizable<'t> = 't * Volumn

type Inbox<'t> = 't IInventory
type Outbox<'t> = 't IInventory
type Transfer<'t> = ('t Outbox * 't Quantitizable) * 't Inbox

[<AutoOpen>]
module Inventory =
  let processTransfer (((src, (item, volumn)), dest) : 't Transfer) = 
    dest.Load(src.Unload(item, volumn))
  let inline (->>) x y = processTransfer (x, y)
  let inline (<=>) (x : IInventory<'x>) (y : IInventory<'y>) = x.GetType() = y.GetType()
  let mergeTransfers : 't Transfer seq -> 't Transfer seq = fun transfers ->
    transfers
    |> Seq.groupBy (fun ((o, (m, v)), i) -> o, m, i)
    |> Seq.map (fun ((o, m, i), g) ->
      let v =  g |> Seq.sumBy (fun ((_, (_, v)), _) -> v)
      ((o, (m, v)), i)
    )
  let toPackage : Package<'t> -> Package = fun (source, (item, volumn)) ->
    let source = source :> IInventory
    let items = item :> obj, volumn

    source, items    
  let toPackageof<'t> : Package -> Package<'t> = fun (source, (item, volumn)) ->
    let source = source :?> 't IInventory
    let items = item :?> 't, volumn

    source, items