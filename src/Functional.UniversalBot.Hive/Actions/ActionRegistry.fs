module HiveActionRegistry

open ConfigurationTypes
open Lamar
open Pipeline

type ActionRegistry () as self =
    inherit ServiceRegistry ()
    do 
        self.For<Binder>().Use(FlushTokens.bind).Named("flush") |> ignore
        self.For<Binder>().Use(ReadPosts.bind).Named("read_posts") |> ignore
        self.For<Binder>().Use(CommentOnPosts.bind).Named("comment_on_posts") |> ignore
        self.For<Binder>().Use(VoteOnPosts.bind).Named("vote_on_posts") |> ignore
        self.For<Binder>().Use(LoadTemplate.bind).Named("load_template") |> ignore
        self.For<Binder>().Use(Variable.bind).Named("variable") |> ignore
