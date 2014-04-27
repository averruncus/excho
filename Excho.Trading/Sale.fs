namespace Excho.Trading

open System
open Unchecked
open Excho.Events
open Excho.Data
open Excho.Logistics

type Sale<'m, 'p> = {
  Id : int
  Merchandise : 'm Package
  Price : 'p
  PayChannel : 'p IInventory
  Status : SaleStatus
  PublishedDate : DateTime
  ExpirationDate : DateTime
  CreatedBy : IActor
  CreatedDate : DateTime
  ModifiedDate : DateTime
}
with
  interface IExchange<'m, 'p> with
    member this.Merchandise =
      let outbox, (m, v) = this.Merchandise
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

[<RequireQualifiedAccess>]
module Sale = 
  let private wrap<'m, 'p> (sale : Repository.Provider.Sale, acc : Repository.Provider.Account) =
    defaultof<Sale<'m, 'p>>
  let getByOwner (owner : IActor) = 
    let sales = dbContainers.Sales
    let accounts = dbContainers.Accounts
    query {
      for sale in sales do
      join account in accounts on (sale.CreatedBy = account.Id)
      where (account.Id = owner.Id)
      select (sale, account)
    }
    |> Seq.map wrap
  let ordinarySale =
    ()
  let freighterOf<'t when 't :> IInventory> = ()
  ()