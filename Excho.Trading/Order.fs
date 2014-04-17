namespace Excho.Trading

open System
open Unchecked

open Exchange

type Order<'m, 'p> = {
  Sales : (Sale<'m, 'p> * BuyExchangeChannel<'p, 'm> option) seq
  Status : OrderStatus
  CreatedDate : DateTime
  ModifiedDate : DateTime
}
with
  interface IExchange<'m, 'p> with
    member this.Merchandise 
      with get() =
        this.Sales 
        |> Seq.map (fun (s, _) ->
          let e = s :> IExchange<_, _>
          e.Merchandise
        )
        |> Seq.concat
    member this.Price
      with get() = 
        this.Sales 
        |> Seq.map (fun (s, channel) ->
          let e = s :> IExchange<_, _>
          e.Price
          |> Seq.map (fun (qp, (ot, it)) -> 
            let o, i = 
              match channel with 
              | Some (o, i) -> o, i 
              | _ -> ot, it
            qp, (o, i)
          )
        )
        |> Seq.concat
and BuyExchangeChannel<'m, 'p> = ExchangeChannel<'m, 'p>
and OrderStatus =
  | Requested = 0
  | Paid = 1
  | Delivered = 2

module Order =
  let save order = order
  let pay payment order =
    save {
      order with
        Sales =
          order.Sales
          |> Seq.zip payment
          |> Seq.map (fun (cp, (s, _)) -> s, Some cp)
        Status = OrderStatus.Paid
    }
    |> processExchange
