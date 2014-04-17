namespace Excho.Data

open System.Configuration
open System.Data
open System.Data.Entity
open Microsoft.FSharp.Data.TypeProviders
open System.Data.EntityClient
open System.Data.Metadata.Edm
open System.Data.Common

[<AutoOpen>]
module internal DbExts =
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

type internal Repository = EdmxFile<"excho.edmx", ResolutionFolder = "../Excho.Data.Models/">

[<AutoOpen>]
module InternalDb =
  let private getModels() = new Repository.Provider.Entities(Db.getDefaultConnection())
  let mutable private models = getModels()
  let reset() = models <- getModels()

  type Repository with
    static member internal Models = models

