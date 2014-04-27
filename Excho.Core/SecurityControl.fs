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
      let p = Thread.CurrentPrincipal :?> ExchoPrincipal
      p.Principal

  let tryAuthenticate id factor = None

  let authenticate id factor =
    match tryAuthenticate id factor with
    | Some actor -> 
      Thread.CurrentPrincipal <- { Principal = actor; Authorities =  actor.Authorities }
      actor
    | _ -> raise (AuthenticationException())