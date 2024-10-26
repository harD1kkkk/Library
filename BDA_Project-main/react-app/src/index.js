import "./index.scss";
import React from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import router from "./pages/routes/routes";
import { Provider } from "react-redux";
import { ContextProvider } from "./contexts/ContextProvider";
import store from "./store/store";

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <Provider store={store}>
    <ContextProvider>
      <RouterProvider router={router} />
    </ContextProvider>
  </Provider>
);
