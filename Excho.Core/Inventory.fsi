namespace Excho.Market

type IInventory =
  abstract member Unload : obj * Volumn -> IPackage
  abstract member Load : IPackage -> unit
and IInventory<'t> =
  inherit IInventory
  abstract member Unload : 't * Volumn -> IPackage<'t>
  abstract member Load : IPackage<'t> -> unit
and IPackage =
  abstract member Source : IInventory
  abstract member Items : obj * Volumn
and IPackage<'t> =
  inherit IPackage
  abstract member Source : IInventory<'t>
  abstract member Items : 't * Volumn
and Volumn = int

type Inbox<'t> = 't IInventory
type Outbox<'t> = 't IInventory
type Quantitizable<'t> = 't * Volumn
type Transfer<'t> = ('t Outbox * 't Quantitizable) * 't Inbox

module Inventory =
  val inline transferTo<'t> : 't Inbox -> ('t Outbox * 't Quantitizable) -> unit
  val inline (->>) : ('t Outbox * 't Quantitizable) -> 't Inbox -> unit
  val inline (<<-) : 't Inbox -> ('t Outbox * 't Quantitizable) -> unit
  val inline (<=>) : IInventory<'x> -> IInventory<'y> -> bool
  val mergeTransfers<'t when 't : equality> : 't Transfer seq -> 't Transfer seq