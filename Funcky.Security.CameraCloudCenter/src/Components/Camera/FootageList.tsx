import React from "react";
import {Footage, FootageUrl} from "../../Models";
import {AjaxService} from "../../Services";

interface FootageListProps
{
    footage: Footage;
    cameraName: string;
    nextFootage: () => void;
    previousFootage: () => void;
}

interface FootageListState
{
    currentFootageIndex: number;
    footageUrl?: FootageUrl;
}

export class FootageList extends React.Component<FootageListProps, FootageListState>
{
    constructor(props: FootageListProps)
    {
        super(props);

        this.state = {
            currentFootageIndex: 0
        };
    }

    componentDidMount()
    {
        this.loadCurrentFootage(); 
    }

    componentWillReceiveProps(nextProps: FootageListProps)
    {
        if (this.props.footage.id !== nextProps.footage.id)
        {
            this.setState({currentFootageIndex: 0, footageUrl: undefined});
            this.loadCurrentFootage();
        }
    }

    private loadCurrentFootage()
    {
        const currentFootage = this.props.footage.sequences[this.state.currentFootageIndex];
        AjaxService.get('api/footage/' + this.props.cameraName + '?id=' + currentFootage.id).then((data: FootageUrl) =>
        {
            this.setState({
                footageUrl: data
            });
        });
    }

    render()
    {
        const currentFootage = this.props.footage.sequences[this.state.currentFootageIndex];
        let details = <div className="card">
            <img className="card-img-top" src="/img/1280x720.gif" alt="No footage selected"/>
            
            {this.getFootageDetail()}
            
            <div className="card-body">
                <div>
                        <span className="badge badge-primary">
                            {this.state.currentFootageIndex + 1} / {this.props.footage.sequences.length}
                        </span>
                </div>
                <div>
                    <b>
                        {currentFootage.title}
                    </b>
                </div>
                <div className="row">
                    <div className="col-6">
                        <button type="button" className="btn btn-secondary w-100 p-2"
                                onClick={(e) => this.moveFootage(e, -1)}>Previous
                        </button>
                    </div>
                    <div className="col-6">
                        <button type="button" className="btn btn-primary w-100 p-2"
                                onClick={(e) => this.moveFootage(e, 1)}>Next
                        </button>
                    </div>
                </div>
            </div>
        </div>;

        if (this.state.footageUrl)
        {
            details = <div className="card">
                <img className="card-img-top" src={this.state.footageUrl.url} alt={currentFootage.title}/>
                <div className="card-body">
                    <div>
                        <span className="badge badge-primary">
                            {this.state.currentFootageIndex + 1} / {this.props.footage.sequences.length}
                        </span>
                    </div>
                    <div>
                        <b>
                            {currentFootage.title}
                        </b>
                    </div>
                    <div className="row">
                        <div className="col-6">
                            <button type="button" className="btn btn-secondary w-100 p-2"
                                    onClick={(e) => this.moveFootage(e, -1)}>Previous
                            </button>
                        </div>
                        <div className="col-6">
                            <button type="button" className="btn btn-primary w-100 p-2"
                                    onClick={(e) => this.moveFootage(e, 1)}>Next
                            </button>
                        </div>
                    </div>
                </div>
            </div>;
        }

        return <>
            <div>
                <h3>
                    {this.props.footage.title}
                </h3>

                {details}


            </div>
        </>;
    }

    private moveFootage(e: React.MouseEvent<HTMLButtonElement>, jump: number)
    {
        e.preventDefault();

        const index = this.state.currentFootageIndex + jump;

        if (index < 0)
        {
            this.props.previousFootage();
            return;
        }

        if (index >= this.props.footage.sequences.length)
        {
            this.props.nextFootage();
            return;
        }
        
        this.setState({currentFootageIndex: index});
        this.loadCurrentFootage();
    }

    private getFootageDetail()
    {
        let content = <></>;
        
        if (!this.state.footageUrl)
        {
            return content;
        }
        
        switch (this.state.footageUrl.type)
        {
            case "snap":
                content = <img className="w-100" src={this.state.footageUrl.url} alt={this.props.footage.title}/>;
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
        
        return content;
    }
}