import React, { Component } from 'react';
import { EditorContent, Editor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import TextAlign from '@tiptap/extension-text-align';
import Underline from '@tiptap/extension-underline';

class EditorTexto extends Component {

    constructor(props) {
        super(props);
        this.editor = null;
    }

    componentDidMount() {
        setTimeout(() => {
            this.editor = new Editor({
                extensions: [
                    StarterKit,
                    Underline,
                    TextAlign.configure({ types: ['heading', 'paragraph'] }),
                ],
                content: this.props.value || '',
                onUpdate: ({ editor }) => {
                    if (this.props.onChange) {
                        this.props.onChange(editor.getHTML());
                    }
                    this.forceUpdate();
                },
                onSelectionUpdate: () => {
                    this.forceUpdate();
                },
                onTransaction: () => {
                    this.forceUpdate();
                }
            });
            this.forceUpdate();
        }, 500);
    }

    componentWillUnmount() {
        if (this.editor) this.editor.destroy();
    }

    render() {
        if (!this.editor) return null;

        const ed = this.editor;

        return (
            <div style={{ border: '1px solid #ccc', borderRadius: 4 }}>

                {/* Toolbar */}
                <div style={{ display: 'flex', gap: 4, padding: '6px 8px', borderBottom: '1px solid #ccc', flexWrap: 'wrap' }}>

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleBold().run()}
                        style={{ fontWeight: 'bold', border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('bold') ? '#e0e0e0' : '#fff' }}>
                        N
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleItalic().run()}
                        style={{ fontStyle: 'italic', border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('italic') ? '#e0e0e0' : '#fff' }}>
                        I
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleUnderline().run()}
                        style={{ textDecoration: 'underline', border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('underline') ? '#e0e0e0' : '#fff' }}>
                        S
                    </button>

                    <span style={{ borderLeft: '1px solid #ccc', margin: '0 4px' }} />

                    <button type="button"
                        onClick={() => ed.chain().focus().setTextAlign('left').run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive({ textAlign: 'left' }) ? '#e0e0e0' : '#fff' }}>
                        ◀≡
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().setTextAlign('center').run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive({ textAlign: 'center' }) ? '#e0e0e0' : '#fff' }}>
                        ≡
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().setTextAlign('right').run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive({ textAlign: 'right' }) ? '#e0e0e0' : '#fff' }}>
                        ≡▶
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().setTextAlign('justify').run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive({ textAlign: 'justify' }) ? '#e0e0e0' : '#fff' }}>
                        ☰
                    </button>

                    <span style={{ borderLeft: '1px solid #ccc', margin: '0 4px' }} />

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleBulletList().run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('bulletList') ? '#e0e0e0' : '#fff' }}>
                        • Lista
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleOrderedList().run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('orderedList') ? '#e0e0e0' : '#fff' }}>
                        1. Lista
                    </button>

                    <span style={{ borderLeft: '1px solid #ccc', margin: '0 4px' }} />

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleHeading({ level: 1 }).run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('heading', { level: 1 }) ? '#e0e0e0' : '#fff' }}>
                        H1
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().toggleHeading({ level: 2 }).run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: ed.isActive('heading', { level: 2 }) ? '#e0e0e0' : '#fff' }}>
                        H2
                    </button>

                    <span style={{ borderLeft: '1px solid #ccc', margin: '0 4px' }} />

                    <button type="button"
                        onClick={() => ed.chain().focus().undo().run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: '#fff' }}>
                        ↩
                    </button>

                    <button type="button"
                        onClick={() => ed.chain().focus().redo().run()}
                        style={{ border: '1px solid #ccc', borderRadius: 3, padding: '2px 8px', background: '#fff' }}>
                        ↪
                    </button>

                </div>

                {/* Área de edição */}
                <div
                    style={{ padding: '12px', minHeight: 300, cursor: 'text' }}
                    onClick={() => ed.commands.focus()}
                >
                    <EditorContent editor={this.editor} />
                </div>

            </div>
        );
    }
}

export default EditorTexto;