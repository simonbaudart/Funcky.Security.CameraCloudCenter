import React from "react";

import { Menu, SideBar } from "./Components/Navigation";
import { ContextContent } from "./Models";
import { ContextProvider } from "./Components/Shared";

interface ApplicationState {
    context: ContextContent;
}

export class Application extends React.Component<any, ApplicationState>
{
    constructor(props: any) {
        super(props);

        this.state = {
            context: {
                route: document.location.hash.replace("#", "") || "/",
                setRoute: this.setRoute.bind(this),
                updateContext: this.updateContext.bind(this),
            }
        };
    }

    public updateContext(context: ContextContent) {
        this.setState({ context: context });
    }

    public setRoute = (route: string) => {
        window.location.href = '#' + route;
        var context = this.state.context;
        context.route = route;
        this.updateContext(context);
    };

    public render(): Object | string | number | {} | Object | Object | boolean | null | undefined {
        return <ContextProvider value={this.state.context}>
            <div className="container-fluid">
                <Menu />
                <SideBar />
            </div>
        </ContextProvider>;
    }
}