namespace Excho.Data

open System.Configuration
open System.Data
open System.Data.Entity
open Microsoft.FSharp.Data.TypeProviders
open System.Data.EntityClient
open System.Data.Metadata.Edm
open System.Data.Common

type internal Repository = EdmxFile<"excho.edmx", ResolutionFolder = "../Excho.Data.Models/">

[<AutoOpen>]
module InternalDb =
  let private getModels() = new Repository.Provider.Entities(Db.getDefaultConnection())
  let mutable private models = getModels()
  let reset() = models <- getModels()

  type Repository with
    static member internal Models = models

