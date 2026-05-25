import React, { useEffect, useState, useRef } from 'react';
import ReactDOM from 'react-dom';

function OffCanvas({children, closeCallback, title}){

    const offCanvasModalRef = useRef(null);
    const divBackdropRef = useRef(null);
     
    useEffect(() => {

        if (document.getElementsByClassName("bs-canvas-overlay").length > 0)
        {
            divBackdropRef.current = document.getElementsByClassName("bs-canvas-overlay")[0];
        }
        else divBackdropRef.current = document.createElement("div");

        divBackdropRef.current.classList.add("bs-canvas-overlay")

        setTimeout(() => {

            divBackdropRef.current.classList.add("show")
        }, 10)
        
       
        divBackdropRef.current.onclick = close;
 
        offCanvasModalRef.current.parentNode.appendChild(divBackdropRef.current);
        offCanvasModalRef.current.classList.add("mr-0");

       
    }, []);


    
    const close = () => {


        offCanvasModalRef.current.classList.remove("mr-0");
        offCanvasModalRef.current.classList.remove("ml-0");
        
        offCanvasModalRef.current.parentNode.removeChild(divBackdropRef.current);

        divBackdropRef.current = null;

        if (closeCallback != null)
        {
            setTimeout(() => {
                closeCallback();
            }, 500)

        }
      
    }

    let saida = 
        <div className="bs-canvas bs-canvas-right position-fixed h-100" ref={offCanvasModalRef}>
            <header className="bs-canvas-header">
                <h4 className="d-inline-block mb-0 float-left">{title}</h4>
                <button type="button" className="bs-canvas-close float-right close" aria-label="Close" onClick={close} ><span aria-hidden="true">&times;</span></button>
            </header>
            <div className="bs-canvas-content">
                {children}
            </div>    
        </div>


   
    return saida;
}

export default OffCanvas;
