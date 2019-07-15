import React from "react";

import { ContextContent } from "../../Models/ContextContent"

const Context = React.createContext<ContextContent | null>(null);

export const ContextProvider = Context.Provider;
export const ContextConsumer = Context.Consumer;