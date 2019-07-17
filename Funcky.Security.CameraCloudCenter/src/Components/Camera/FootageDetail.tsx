import React from "react";
import { Footage } from "../../Models";

export interface FootageDetailProps
{
    footage: Footage | undefined;
}

export const FootageDetail = (props: FootageDetailProps) =>
{
    let content = <div></div>;

    if (props.footage)
    {
        content = <div className="card">
                      <div className="card-header" id="headingThree">
                          <h5 className="mb-0">
                              <button className="btn btn-link collapsed" data-toggle="collapse" data-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                                  {props.footage.title}
                              </button>
                          </h5>
                      </div>
                      <div id="collapseThree" className="collapse" aria-labelledby="headingThree" data-parent="#accordion">
                          <div className="card-body">
                              Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor, sunt aliqua put a bird on it squid single-origin coffee nulla assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic synth nesciunt you probably haven't heard of them accusamus labore sustainable VHS.
                          </div>
                      </div>
                  </div>;
    }

    return content;
}