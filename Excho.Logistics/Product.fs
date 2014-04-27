namespace Excho.Logistics

open System
open Excho
open Excho.Data
open Excho.Events
open Excho.Security

type Product = {
  Id : int
  Name : string
  Image : string
  PreviewImages : string seq
  Description : string
  CreatedBy : IActor
  CreatedDate : DateTime
  ModifiedDate : DateTime
  Data : obj
  Repository : IRepository
}
with
  interface IPersistable with
    member this.Data = this.Data
    member this.Repository = this.Repository
and private ProductRepo() = inherit Repository<DbProduct>(dbContainers.Products)

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Product =
  let internal wrap (p : DbProduct, repo) =
    {
      Id = p.Id
      Name = p.Name
      Image = ""
      PreviewImages = Seq.empty
      Description = ""
      CreatedBy = Actor.CurrentPrincipal
      CreatedDate = p.CreatedDate
      ModifiedDate = p.ModifiedDate ?|| DateTime.MinValue
      Data = p
      Repository = repo
    }