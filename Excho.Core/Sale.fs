namespace Excho.Market

open System
open Unchecked

type Sale<'m, 'p> = {
  Merchandise : 'm * Volumn * Outbox<'m>
  Price : 'p
  PayChannel : IInventory<'p>
  Status : SaleStatus
  BeginDate : DateTime
  EndDate : DateTime
}
with
  interface IExchange<'m, 'p> with
    member this.Merchandise =
      let m, v, outbox = this.Merchandise
      let inbox = this.PayChannel
      [| (m, v), (outbox, inbox) |] :> _ seq
    member this.Price = 
      let p = this.Price
      let v = 1
      let outbox = this.PayChannel
      let inbox = defaultof<IInventory<'m>>
      [| (p, v), (outbox, inbox) |] :> _ seq
and QuantitizedSale<'m, 'p> = Sale<'m, 'p> * Volumn
and SaleStatus =
  | Bargaining = 0
  | Dealed = 1

module Sale = 
  let ordinarySale = ()
  let bidding = ()
  let tendering = ()
  let freighterOf<'t when 't :> IInventory> = ()
  ()