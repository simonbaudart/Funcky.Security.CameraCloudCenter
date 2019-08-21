import React from 'react';

import { withContext, ContextAwareProps } from "../../Hoc";
import { Routes } from "../../Routing";
import { AjaxService } from "../../Services";

import AppActions from "../../Stores/AppActions";

const MenuComponent = (props: ContextAwareProps) =>
{
    const logout = () =>
    {
        AjaxService.postNoData('api/logout').then(() =>
        {
            AppActions.logoutSuccess();
        });
    };

    const getUserLink = () =>
    {
        if (props.context.route === Routes.login)
        {
            return <></>;
        }

        return <li className="nav-item active">
            <a className="nav-link" href="#" onClick={(e) =>
            {
                e.preventDefault();
                logout();
            }}>
                Logout
            </a></li>;
    };

    return <nav className="navbar navbar-expand-lg navbar-light bg-light">
        <a className="navbar-brand" href="#">Camera Cloud Center</a>

        <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent"
            aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon">
            </span>
        </button>

        <div className="collapse navbar-collapse" id="navbarSupportedContent">
            <ul className="navbar-nav mr-auto">
                <li className="nav-item active">
                    <a className="nav-link" href="#">Dashboard</a>
                </li>
                {getUserLink()}
            </ul>
        </div>


    </nav>;
};

export const Menu = withContext<ContextAwareProps>(MenuComponent);