import React from "react";

import { Menu, SideBar } from "./Components/Navigation";
import { Camera, ContextContent } from "./Models";
import { ContextProvider } from "./Components/Shared";
import { AjaxService } from "./Services";

interface ApplicationState {
    context: ContextContent;
    cameras: Camera[];
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
            },
            cameras: []
        };
    }

    public componentDidMount()
    {
        AjaxService.get<Camera[]>('/api/cameras').then((data) =>
        {
            this.setState({ cameras: data });
        });
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
                <div className="row pb-3">
                    <div className="col">
                        <Menu />
                    </div>
                </div>

                <div className="row pb-3">
                    <div className="col-2">
                        <SideBar cameras={this.state.cameras} />
                    </div>
                </div>
                
            </div>
        </ContextProvider>;
    }
}