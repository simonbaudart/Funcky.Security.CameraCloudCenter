import React from "react";
import {Footage, FootageUrl} from "../../Models";
import {AjaxService} from "../../Services";

export interface FootageDetailProps
{
    footage: Footage;
    cameraName: string;
}

export interface FootageDetailState
{
    footageUrl?: FootageUrl;
}

export class FootageDetail extends React.Component<FootageDetailProps, FootageDetailState>
{
    constructor(props)
    {
        super(props);

        this.state = {};
    }

    showFootage()
    {
        if (this.state.footageUrl)
        {
            return;
        }

        AjaxService.get('api/footage/' + this.props.cameraName + '?id=' + this.props.footage.id).then((data: FootageUrl) =>
        {
            this.setState({
                footageUrl: data
            });
        });
    }

    render()
    {
        let content = <></>;

        if (this.state.footageUrl)
        {
            switch (this.state.footageUrl.type)
            {
                case "snap":
                    content = <div>
                        <img className="w-100" src={this.state.footageUrl.url} alt={this.props.footage.title}/>
                        <a href={this.state.footageUrl.url}>{this.props.footage.title}</a>
                    </div>;
                    break;
                case "recording":
                    content = <div>
                        <video className="w-100" src={this.state.footageUrl.url} autoPlay controls
                               onError={(error: any) => console.log(error.target.error)}>
                        </video>
                        <a href={this.state.footageUrl.url}>{this.props.footage.title}</a>
                    </div>;
                    break;
                default:
                    content = <div>{this.state.footageUrl.url}</div>;
                    break;
            }
        }

        return <div className="card">
            <div className="card-header" id={"heading" + this.props.footage.id}>
                <h5 className="mb-0">
                    <button className="btn btn-link collapsed" onClick={() => this.showFootage()} data-toggle="collapse"
                            data-target={"#" + this.props.footage.id} aria-expanded="false"
                            aria-controls="collapseThree">
                        {this.props.footage.title}
                    </button>
                </h5>
            </div>
            <div id={this.props.footage.id} className="collapse" aria-labelledby={"heading" + this.props.footage.id}
                 data-parent="#accordion">
                <div className="card-body">
                    {content}
                </div>
            </div>
        </div>;
    }
}