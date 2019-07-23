import React from "react";
import { ContextAwareProps } from "../Hoc/ContextAwareProps";
import { withContext } from "../Hoc/WithContext";

interface RouteProps extends ContextAwareProps
{
    path: string;
    children: React.ReactNode;
}

const RouteComponent = (props: RouteProps) =>
{
    if (props.context.route.startsWith(props.path) || props.path === "/" && (props.context.route === "" || props.context.route === "#"))
    {
        return <React.Fragment>{props.children}</React.Fragment>;
    }

    return null;
};

export const Route = withContext<RouteProps>(RouteComponent);