import React from "react";

export const SideBar = () =>
{
    return <div className="sidebar-fixed position-fixed">
               <ul className="list-group list-group-flush">
                   <a className="activeclassName" aria-current="page" href="/previews/free-templates/react-admin-dashboard/">
                       <li className="list-group-item">
                           <i className="fa fa-pie-chart mr-3"></i>Dashboard
                       </li>
                   </a>
                   <a href="/previews/free-templates/react-admin-dashboard/profile">
                       <li className="list-group-item"><i className="fa fa-user mr-3"></i>Profile</li>
                   </a>
                   <a href="/previews/free-templates/react-admin-dashboard/tables">
                       <li className="list-group-item"><i className="fa fa-table mr-3"></i>Tables</li>
                   </a>
                   <a href="/previews/free-templates/react-admin-dashboard/maps">
                       <li className="list-group-item"><i className="fa fa-map mr-3"></i>Maps</li>
                   </a>
                   <a href="/previews/free-templates/react-admin-dashboard/404">
                       <li className="list-group-item"><i className="fa fa-exclamation mr-3"></i>404</li>
                   </a>
               </ul>
           </div>;
};