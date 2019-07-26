import React from "react";

import {ContextContent} from "./Models";
import {ContextProvider, Menu} from "./Components";
import {Route, Routes} from "./Routing";

import {AuthenticationPanel, CameraPanel} from "./Containers";

interface ApplicationState
{
    context: ContextContent;
}

export class Application extends React.Component<any, ApplicationState>
{
    constructor(props: any)
    {
        super(props);

        this.state = {
            context: {
                route: document.location.hash.replace("#", "") || "/",
                setRoute: this.setRoute.bind(this),
                updateContext: this.updateContext.bind(this),
            }            
        };
    }

    public componentDidMount()
    {
    }

    public updateContext(context: ContextContent)
    {
        this.setState({context: context});
    }

    public setRoute = (route: string) =>
    {
        window.location.href = `#${route}`;
        const context = this.state.context;
        context.route = route;
        this.updateContext(context);
    };

    public render()
    {
        return <ContextProvider value={this.state.context}>
            <div className="container-fluid">
                <div className="row pb-3">
                    <div className="col">
                        <Menu/>
                    </div>
                </div>

                <Route path={Routes.dashboard}>
                    <CameraPanel />
                </Route>

                <Route path={Routes.login}>
                    <AuthenticationPanel/>
                </Route>
            </div>
        </ContextProvider>;
    }
}