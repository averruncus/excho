namespace Excho.Data

open System.Collections
open System.Collections.Generic
open System.Data.Objects

[<AllowNullLiteral>]
type IRepository =
  abstract member All : obj seq
  abstract member Commit : obj -> unit
  abstract member Delete : obj -> unit

type RepositoryImpl<'t when 't : not struct> = 't IObjectSet

type Repository<'t when 't : not struct>(impl : 't RepositoryImpl) =
  member this.Impl = impl
  abstract member All : 't seq
  default this.All = impl :> seq<_>
  member this.Commit(data) = this.Impl.AddObject(data)
  member this.Delete(data) = this.Impl.DeleteObject(data)
  member this.GetEnumerator() = this.All.GetEnumerator()
  interface IRepository with
    member this.All = this.All :?> seq<_>
    member this.Commit data = 
      let data = data :?> 't
      this.Commit data
    member this.Delete data =
      let data = data :?> 't
      this.Delete data
  interface IEnumerable<'t> with
    member this.GetEnumerator() = this.GetEnumerator()
    member this.GetEnumerator() = this.GetEnumerator() :> IEnumerator