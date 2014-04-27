namespace Excho.Data

open System.Collections
open System.Collections.Generic
open System.Data.Objects

[<AbstractClass>]
type RelationalRepository<'p, 's when 'p : not struct and 's : not struct>
  (primaryImpl : 'p RepositoryImpl, secondaryImpl : 's RepositoryImpl) =
  member this.PrimaryImpl = primaryImpl
  member this.SecondaryImpl = secondaryImpl
  abstract member All : ('p * 's) seq
  member this.Comit(p, s) = 
    this.PrimaryImpl.AddObject(p)
    this.SecondaryImpl.AddObject(s)
  member this.Delete(p, s) =
    this.PrimaryImpl.DeleteObject(p)
    this.SecondaryImpl.DeleteObject(s)
  member this.GetEnumerator() = this.All.GetEnumerator()
  interface IRepository with
    member this.All = this.All :?> seq<_>
    member this.Commit data =
      let data = data :?> ('p * 's)
      this.Comit(data)
    member this.Delete data =
      let data = data :?> ('p * 's)
      this.Delete(data)
  interface IEnumerable<'p * 's> with
    member this.GetEnumerator() = this.GetEnumerator()
    member this.GetEnumerator() = this.GetEnumerator() :> IEnumerator