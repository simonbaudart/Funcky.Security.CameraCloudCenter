import "react-app-polyfill/ie9";
import "react-app-polyfill/stable";

import React from "react";
import ReactDOM from "react-dom";

import { Application } from "./Application";

ReactDOM.render(<Application/>, document.getElementById("application"));