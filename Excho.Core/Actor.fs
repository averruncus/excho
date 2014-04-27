namespace Excho.Events

open System
open System.Data.Objects
open Excho.Data

[<AllowNullLiteral>]
type IActor =
  abstract Id : int
  abstract Name : string

type Actor = {
  Id : int
  Name : string
  Data : obj
  Repository : IRepository
}
with
  interface IActor with
    member this.Id = this.Id
    member this.Name = this.Name
  interface IPersistable with
    member this.Data = this.Data
    member this.Repository = this.Repository

[<AutoOpen>]
module ActorExts =
  let actor = { Id = 0; Name = String.Empty; Data = null; Repository = null }
