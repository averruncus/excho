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
module internal InternalDb =
  type internal Types = Repository.Provider
  type internal Containers = Repository.Provider.Entities
  let private getDbContainers() = new Repository.Provider.Entities(Db.getDefaultConnection())
  let mutable private _dbContainers = getDbContainers()
  let dbContainers = _dbContainers
  let reset() = _dbContainers <- getDbContainers()