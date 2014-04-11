namespace Excho.Market

open System
open Unchecked
open Inventory

type ProductInventory = {
  Id : int
  Unload : Product * Volumn -> Product Package
  Load : Product Package -> unit
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ProductInventory =
  let private defaultInventory = defaultof<ProductInventory>
  let cargoHold =
    {
      defaultInventory with
        Load = fun items -> ()
        Unload = fun items -> Unchecked.defaultof<Product Package>
    }
  let freighter =
    {
      defaultInventory with
        Load = fun items -> ()
        Unload = fun items -> Unchecked.defaultof<Product Package>
    }
      