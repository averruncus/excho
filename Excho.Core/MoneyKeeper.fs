namespace Excho.Market

open System
open Unchecked
open Inventory

type MoneyKeeper = {
  Id : string
  Owner : string
  Currency : string
  Load : Money Package -> unit
  Unload : double -> Money Package
}
with
  interface IInventory<Money> with
    member this.Unload(m : Money, v : Volumn) =
      this.Unload(m.Amount * double v)
    member this.Unload(m : obj, v : Volumn) = 
      let this = this :> IInventory<Money>
      this.Unload(m :?> Money, v) |> toPackage
    member this.Load(package) = this.Load(package)
    member this.Load(package : Package) =
      let package = package |> toPackageof<Money>
      this.Load(package)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MoneyKeeper =
  let private defaultKeeper = defaultof<MoneyKeeper>
  let creditCard =
    {
      defaultKeeper with
        Load = fun m -> ()
        Unload = fun m -> defaultof<Money Package>
    }
  let paypal =
    {
      defaultKeeper with
        Load = fun m -> ()
        Unload = fun m -> defaultof<Money Package>
    }