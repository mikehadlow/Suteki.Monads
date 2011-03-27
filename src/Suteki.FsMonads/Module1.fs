// Learn more about F# at http://fsharp.net

module Module1

type Identity<'a> = Identity of 'a

let getValue (a : Identity<'a>) = match a with Identity x -> x
let mreturn x = Identity x
let bind (a : Identity<'a>) (f : 'a -> Identity<'b>) = f (getValue a)

type IdentityBuilder() =
    member x.Bind(a, f) = bind a f
    member x.Return(a) = mreturn a

