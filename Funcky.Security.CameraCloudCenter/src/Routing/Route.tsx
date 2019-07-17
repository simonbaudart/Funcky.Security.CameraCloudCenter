import React from "react";
import { ContextAwareProps } from "../Hoc/ContextAwareProps";
import { withContext } from "../Hoc/WithContext";

interface RouteProps extends ContextAwareProps
{
    path: string;
    children: React.ReactNode;

    roles?: string[];
}

const RouteComponent = (props: RouteProps) =>
{
    // TODO : Vérifier les droits de l'utilisateur

    // Si on est sur la page courante
    if (props.context.route.startsWith(props.path) || props.path === "/" && (props.context.route === "" || props.context.route === "#"))
    {

        return <>{props.children}</>;
    }

    return null;
};

export const Route = withContext<RouteProps>(RouteComponent);