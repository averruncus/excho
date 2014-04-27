﻿namespace Excho.Data

open System.Configuration
open System.Data
open System.Collections
open System.Collections.Generic
open System.Data.Objects
open System.Data.Entity
open Microsoft.FSharp.Data.TypeProviders
open System.Data.EntityClient
open System.Data.Metadata.Edm
open System.Data.Common

type IPersistable =
  abstract member Repository : IRepository
  abstract member Data : obj

[<AutoOpen>]
module private InternalDbExts =
  [<Literal>]
  let CON_STR_NAME = "excho"
  type ConfigurationManager with
    static member ExchoConnectionString = ConfigurationManager.ConnectionStrings.[CON_STR_NAME]

[<RequireQualifiedAccess>]
module Db =
  let getConnection (connectionSettings : ConnectionStringSettings) =
    let dbConnection = DbProviderFactories.GetFactory(connectionSettings.ProviderName).CreateConnection()

    dbConnection.ConnectionString <- connectionSettings.ConnectionString

    let resourceArray = [| "res://*/" |]
    let assemblyList = [| System.Reflection.Assembly.GetCallingAssembly() |]
    let metaData = MetadataWorkspace(resourceArray, assemblyList)

    new EntityConnection(metaData, dbConnection)
  let getDefaultConnection() = getConnection (ConfigurationManager.ExchoConnectionString)

  let save (data : IPersistable) =
    let c = data.Repository
    c.Commit(data.Data)
  let delete (data : IPersistable) =
    let c = data.Repository
    c.Delete(data.Data)

type internal Repository = EdmxFile<"excho.edmx", ResolutionFolder = "../Excho.Data.Models/">

[<AutoOpen>]
module internal InternalDb =
  type internal Types = Repository.Provider
  type internal Containers = Repository.Provider.Entities
  let private getDbContainers() = new Repository.Provider.Entities(Db.getDefaultConnection())
  let mutable private _dbContainers = getDbContainers()
  let internal dbContainers = _dbContainers
  let reset() = _dbContainers <- getDbContainers()