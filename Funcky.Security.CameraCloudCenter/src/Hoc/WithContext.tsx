// Pour faciliter l'utilisation du state, un HoC est créé
// Il envoie au composant le contexte dans la propriété contexte
// si d'autres propriétés doivent être renvoyées, il faut hériter de MyCphContextAwareProps

import React from "react";

import { ContextConsumer } from "../Components/Shared";
import { ContextAwareProps } from "./";
import { ContextContent } from "../Models";

export const withMyCphContext = <P extends ContextAwareProps>(Component: any) =>
{
    return (props: Pick<P, Exclude<keyof P, keyof ContextAwareProps>>) =>
        <ContextConsumer>
            {(context : any) => <Component {...props} context={context} />}
        </ContextConsumer>;
}