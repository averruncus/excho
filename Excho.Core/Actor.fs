namespace Excho.Events

open System
open Excho.Data

type IActor =
  abstract Id : int
  abstract Name : string

type Actor = {
  Id : int
  Name : string
}
with
  interface IActor with
    member this.Id = this.Id
    member this.Name = this.Name

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Actor =
  let internal wrap (acc : Repository.Provider.Account) =
    {
      Id = acc.Id
      Name = acc.Name
    } :> IActor

