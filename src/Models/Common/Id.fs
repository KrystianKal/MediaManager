namespace MediaManager.Models.Common

open System

[<CustomComparison>]
[<CustomEquality>]
[<Struct>]
type Id<'T> = Id of Guid with
    override this.Equals(obj) =
        match obj with
        | :? Id<'T> as other ->
            let (Id id1) = this
            let (Id id2) = other
            id1.Equals(id2)
        | _ -> false
    override this.GetHashCode() =
        let (Id id ) = this in id.GetHashCode()
    
    interface IComparable<Id<'T>> with
        member this.CompareTo(other) =
            let (Id id1) = this
            let (Id id2) = other
            id1.CompareTo(id2)
            
    interface IComparable with
        member this.CompareTo(other) =
            match other with
            | null -> 1
            | :? Id<'T> as other -> (this :> IComparable<Id<'T>>).CompareTo(other)
            | _ -> invalidArg "obj" $"{other.GetType()} is not a valid Id"
            
module Id =
    let create<'T> () = Id(Guid.NewGuid())
    let value (Id id) = id
    let fromGuid (guid: Guid ) = Id(guid)