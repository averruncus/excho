namespace Excho.Registration

open System
open System.Linq
open System.Data.Objects
open Excho.Data
open Excho
open Excho.Events

type private DbAccount = Repository.Provider.Account
type private DbUser = Repository.Provider.User
type private UserRepo() =
  inherit HierachicalRepository<DbAccount, DbUser>(dbContainers.Accounts, dbContainers.Users)
  override this.All =
    let accs = this.PrimaryImpl
    let users = this.SecondaryImpl
    query {
      for acc in accs do
      join user in users on (acc.Id = user.Id)
      select (acc, user)
    } :> seq<_>

type User = {
  Id : int
  Name : string
  Firstname : string
  Lastname : string
  Image : string
  CreatedDate : DateTime
  ModifiedDate : DateTime
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
    
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module User =
  let internal wrap (a : DbAccount, u : DbUser, repo) =
    {
      Id = a.Id
      Name = a.Name
      Firstname = u.Firstname
      Lastname = u.Lastname
      Image = u.Image
      CreatedDate = a.CreatedDate
      ModifiedDate = u.ModifiedDate ?|| DateTime.MinValue
      Data = a, u
      Repository = repo
    }
  let tryGetById id =
    let repo = UserRepo()
    let lookup =
      query {
        for (a, u) in repo.AsQueryable() do
        where (a.Id = id)
        select (a, u)
        take 1
      }
      |> Seq.map (fun (a, u) -> a, u, repo)
      |> Seq.map wrap
      |> Seq.cache

    if lookup |> Seq.length > 0 then 
      lookup |> Seq.head |> Option.Some 
    else 
      None