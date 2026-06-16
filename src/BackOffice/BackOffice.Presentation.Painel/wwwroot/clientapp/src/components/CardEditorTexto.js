import React, { Component } from 'react';
import { Editor } from '@hugerte/hugerte-react';

export default class CardEditorTexto extends Component {

    uploadImagem = async (blobInfo) => {

        const formData = new FormData();

        formData.append(
            'file',
            blobInfo.blob(),
            blobInfo.filename()
        );

        const response = await fetch(
            '/api/upload',
            {
                method: 'POST',
                body: formData
            }
        );

        const result = await response.json();

        return result.url;
    }

    render() {

        const {
            value,
            onChange,
            onEditorReady,
            height = 600
        } = this.props;

        return (

            <Editor

                value={value || ''}

                init={{

                    height,

                    menubar: true,

                    promotion: false,

                    branding: false,

                    plugins: [

                        'advlist',
                        'autolink',
                        'lists',
                        'link',
                        'image',
                        'charmap',
                        'preview',
                        'anchor',
                        'searchreplace',
                        'visualblocks',
                        'code',
                        'fullscreen',
                        'insertdatetime',
                        'media',
                        'table',
                        'help',
                        'wordcount'

                    ],

                    toolbar:

                        'undo redo | ' +
                        'blocks fontfamily fontsize | ' +
                        'bold italic underline strikethrough | ' +
                        'alignleft aligncenter alignright alignjustify | ' +
                        'bullist numlist outdent indent | ' +
                        'forecolor backcolor | ' +
                        'link image table | ' +
                        'removeformat | ' +
                        'code fullscreen',

                    image_title: true,

                    image_caption: true,

                    image_dimensions: true,

                    automatic_uploads: true,

                    images_upload_handler: async (blobInfo) => {

                        return await this.uploadImagem(blobInfo);

                    },

                    content_style: `
                        body {
                            font-family: Arial, Helvetica, sans-serif;
                            font-size: 14px;
                            margin: 20px;
                        }

                        table {
                            border-collapse: collapse;
                            width: 100%;
                        }

                        table td,
                        table th {
                            border: 1px solid #ccc;
                            padding: 5px;
                        }

                        img {
                            max-width: 100%;
                            height: auto;
                        }
                    `

                }}

                onInit={(evt, editor) => {

                    if (onEditorReady) {

                        onEditorReady(editor);

                    }

                }}

                onEditorChange={(content) => {

                    if (onChange) {

                        onChange(content);

                    }

                }}

            />

        );

    }

}