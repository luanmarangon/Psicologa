import React, { Component } from 'react';

export default class CadastroTipo extends Component {

    constructor(props) {
        super(props);
        this.state = {
            tipo: {
                id: "0",
                tipo: "0",
                tipoNome: "",
                dtContrato: "",
                situacao: "A",
                nroCartao: ""
            }
        };
    }

    componentDidUpdate = () => {
        tableSelectable();
    }

    adicionarTipo = () =>{

        if (parseInt(this.state.tipo.tipo) != 0)
        {
            let tipos = this.props.tipos;
            let item = null;
            if (this.state.tipo.id != "0")
            {
                //edição
                item = tipos.find((item) => {return item.id === this.state.tipo.id});

                if (item != null)
                {
                    item.tipo = this.state.tipo.tipo;
                    item.tipoNome = this.refs.selTp[this.refs.selTp.selectedIndex].innerHTML;
                    item.dtContrato = this.state.tipo.dtContrato;
                    item.situacao = this.state.tipo.situacao;
                    item.nroCartao = this.state.tipo.nroCartao;
                }
            }

            if (item == null)
            {
                if (tipos.findIndex((item) => {return item.tipo === this.state.tipo.tipo}) > -1)
                    return;

                //novo    
                tipos.push({
                    id: new Date().getTime() * -1,
                    tipo: this.state.tipo.tipo,
                    tipoNome: this.refs.selTp[this.refs.selTp.selectedIndex].innerHTML,
                    dtContrato: this.state.tipo.dtContrato,
                    situacao: this.state.tipo.situacao,
                    nroCartao: this.state.tipo.nroCartao
                });
            }
            
            this.setState({
                tipo: {
                    ...this.state.tipo,
                    id: 0,
                    dtContrato: "",
                    situacao: "",
                    nroCartao: ""
                }
            });

            this.props.atualizarState(tipos);
        }
    }


    excluirTipo = (itemExcluir) =>{

        if (!confirm("Confirma a exclusão?"))
        {
            return false;
        }

        let tipos = this.props.tipos.filter(item => {return item.id !== itemExcluir.id})
        this.props.atualizarState(tipos);
    }


    editarTipo = (itemEditar) =>{
          
        this.setState({
            tipo: {
                ...this.state.tipo,
                id: itemEditar.id, 
                tipo: itemEditar.tipo,
                dtContrato: itemEditar.dtContrato,
                situacao: itemEditar.situacao,
                nroCartao: itemEditar.nroCartao
            }
        });
    }
   
    render() {

        let form =
            <div>     

                 <div className="form-group text-info">
                    Uma pessoa pode ter vários tipos, basta clicar em "+".               
                </div>

                 <div className="row">
                    <div className="col-sm-5">   
                        <div className="form-group">
                            <label htmlFor="selTp">Tipo</label>
                            <div className="input-group"> 
                                <select id="selTp" className="form-control" 
                                    value={this.state.tipo.tipo} 
                                    onChange={(e) => this.setState({tipo: {...this.state.tipo, tipo: e.target.value}})}
                                    ref="selTp">
                                    <option value="0"></option>
                                    <option value="1">Paciente</option>
                                    <option value="2">Colaborador</option>
                                    <option value="3">Psicologo</option>
                                </select>   

                                <span className="ml-3">
                                    <button type="button" className="btn btn-primary" onClick={this.adicionarTipo}><i className="fas fa-plus"></i></button>
                                </span> 

                            </div>  
                        </div>
                    </div>
             
                </div>   

                {this.props.tipos.length > 0 &&
                    <div className="card">
                        <div className="card-body table-responsive p-0">
                            <table className="table table-hover table-sm table-selectable">
                                <tbody>
                                    {this.props.tipos.map(item => {
                                        return(
                                            <tr key={item.id}>
                                                <td>
                                                    {item.tipoNome}
                                                </td>
                                                <td style={{width:"50px"}}>
                                                    <div>
                                                        <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                            <i className="action-icon fas fa-ellipsis-v"></i>
                                                        </a>
                                                        <div className="dropdown-menu">
                                                            <a className="dropdown-item" href="#" onClick={(e) => this.excluirTipo(item)}><i className="far fa-trash-alt"></i>Excluir</a>
                                                        </div>
                                                    </div>
                                                </td>

                                            </tr>
                                        );
                                    })}           
                                </tbody>
                            </table>
                        </div>
                    </div>
                }
            </div>

        let card = 
            <div className="card">
                <div className="card-header">
                    <h3 className="card-title">Tipos da Pessoa</h3>
                </div>
                <div className="card-body">
                    {form}
                </div>        
            </div>
        
        return card;
    }
}