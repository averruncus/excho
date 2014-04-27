namespace Excho.Logistics

open System
open System.Linq
open Excho
open Excho.Data
open Excho.Events

type private PossessionRepo() = inherit Repository<DbPossession>(dbContainers.Possessions)

[<RequireQualifiedAccess>]
module ProductPossession =
  let internal wrap : DbPossession * DbProduct * DbInventory * DbAccount -> Product Possession = fun (poss, prod, invt, acc) ->
    let prod : Product Quantitizable = 
      (prod, null) |> Product.wrap, 
      (poss.Volume ?|| 1)
    let invt : Product Source = 
      let s =
        (invt, acc, null) 
        |> ProductInventory.wrap
      s :> Product Source
    
    invt, prod

  let getByOwner (owner : IActor) =
    let possessions = dbContainers.Possessions
    let inventories = dbContainers.Inventories
    let products = dbContainers.Products
    let accounts = dbContainers.Accounts

    query {
      for poss in possessions do
      join prod in products
        on (poss.Property = prod.Id)
      join invt in inventories
        on (poss.Inventory = invt.Id)
      join acc in accounts
        on (invt.Owner = acc.Id)
      where (acc.Id = owner.Id)
      select (poss, prod, invt, acc)
    }
    |> Seq.map wrap
    |> Seq.cache

  let toPersistable ((s, (p, v)) : Product Possession) =
    let s = s :?> ProductInventory
    {
      new IPersistable with
        member this.Data = 
          DbPossession(
            Property = p.Id,
            Inventory = s.Id
          ) :> obj
        member this.Repository = 
          PossessionRepo() :> IRepository
    }