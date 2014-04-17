namespace Excho.Security

open System
open System.Security.Principal
open System.Security.Authentication
open System.Threading
open Excho.Data
open Excho.Events

type private ExchoPrincipal = {
  Principal : IActor
  Authorities : Principal seq
}
with
  interface IPrincipal with
    member this.Identity = GenericIdentity(this.Principal.Name) :> IIdentity
    member this.IsInRole role = this.Authorities |> Seq.exists (fun auth -> auth.Id.ToString() = role)
and Principal = IActor

[<AutoOpen>]
module SecurityControl =
  type IActor with
    member this.Authorities = Seq.empty<Principal>
  type Actor with
    static member CurrentPrincipal =    
      let p =
        Thread.CurrentPrincipal
        |> box
        |> unbox<ExchoPrincipal>
      p.Principal

  let tryAuthenticate id factor =
    let models = Repository.Models
    let lookup =
      query {
        for acc in models.Accounts do
        join auth in models.Authentications on (acc.Id = auth.Account)
        where (auth.Id = id && auth.Factor = factor)
        select acc
        take 1
      }
      |> Seq.map Actor.wrap
      |> Seq.toArray

    if lookup |> (not << Seq.isEmpty) then Some (lookup |> Seq.head) else None

  let authenticate id factor =
    match tryAuthenticate id factor with
    | Some actor -> 
      Thread.CurrentPrincipal <- { Principal = actor; Authorities =  actor.Authorities }
      actor
    | _ -> raise (AuthenticationException())