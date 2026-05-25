import React, { useEffect, useState } from 'react';

function LoadingIndicator(props){

    let timeWait = 10;
    
    if (props.timeWait)
        timeWait = props.timeWait;

    const [showLoading, setShowLoading] = useState(false)

    useEffect(() => {
        let timer1 = setTimeout(() => setShowLoading(true), timeWait);

        // this will clear Timeout when component unmont like in willComponentUnmount
        return () => {
        clearTimeout(timer1)
        }
    }, []);

    let saida = null;


    if (props.full)
    {
        saida = showLoading ?
                <div className="site-loading" style={props.style}>
                    <img src={baseURL + "img/loading.png"} />
                </div>: null;

    }
    else {

        saida = showLoading ?
                <div className="d-flex justify-content-center" style={props.style}>
                    <img style={{width:"30px"}} src={baseURL + "img/loading.png"} />
                </div>: null;    
    }

    return saida;
}

export default LoadingIndicator;
