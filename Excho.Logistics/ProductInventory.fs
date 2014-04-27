namespace Excho.Logistics

open System
open System.Linq
open Unchecked
open Excho
open Excho.Data
open Excho.Events
open Excho.Security

type ProductInventory = {
  Id : int
  Owner : IActor
  Unload : Product * Volumn -> Product Package
  Load : Product Package -> unit
  Data : obj
  Repository : IRepository
}
with
  interface IInventory<Product> with
    member this.Unload(p, v) = this.Unload(p, v)
    member this.Unload(p : obj, v) =     
      this.Unload(p :?> Product, v) |> toPackage
    member this.Load(package) = this.Load(package)
    member this.Load(package : Package) =
      let package = package |> toPackageof<Product>
      this.Load(package)
  interface IPersistable with
    member this.Data = this.Data
    member this.Repository = this.Repository

type private InventoryRepo() = inherit Repository<DbInventory>(dbContainers.Inventories)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ProductInventory =
  let private inventory = defaultof<ProductInventory>
  let cargoHold =
    {
      inventory with
        Load = fun items -> ()
        Unload = fun items -> Unchecked.defaultof<Product Package>
    }
  let freighter =
    {
      inventory with
        Load = fun items -> ()
        Unload = fun items -> Unchecked.defaultof<Product Package>
    }
  let internal wrap (invt : DbInventory, acc : DbAccount, repo) =
    {
      cargoHold with
        Id = invt.Id
        Owner =
          {
            actor with
              Id = acc.Id
              Name = acc.Name
          }
        Data = invt
        Repository = repo
    }
  let getById id =
    let repo = InventoryRepo()
    let inventories = repo.AsQueryable()
    let accounts = dbContainers.Accounts
    let lookup =
      query {
        for invt in inventories do
        join acc in accounts
          on (invt.Owner = acc.Id)
        where (invt.Type = 0uy)
        select (invt, acc)
        take 1
      }
      |> Seq.map (fun (invt, acc) -> invt, acc, repo)
      |> Seq.map wrap
      |> Seq.cache

    if lookup |> Seq.length > 0 then
      lookup |> Seq.head
    else
      raise (Exception())


      