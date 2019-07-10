import React from "react";

import { Menu, SideBar } from "./Components/Navigation";

export class Application extends React.Component<any, any>
{
    public render(): Object | string | number | {} | Object | Object | boolean | null | undefined {
        return <div className="container-fluid">
            <Menu />
            <SideBar />
        </div>;
    }
}