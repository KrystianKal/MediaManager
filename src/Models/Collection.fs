namespace MediaManager.Models

open System

type Collection = {
    Id: Guid
    Name: string
    Items: Set<Media>
    CreatedAt: DateTimeOffset
}

module Collection =
    let create name =
        {
            Id = Guid.NewGuid()
            Name = name
            Items = Set.empty
            CreatedAt = DateTimeOffset.Now 
        }
    
    let addItems items collection =
        {
            collection with Items = Set.union items collection.Items
        }

    let removeItems items collection =
        {
            collection with Items = Set.difference items collection.Items
        }
