import React, { Component } from 'react';

class JsonNode extends Component {

    constructor(props) {
        super(props);
        this.state = { aberto: props.defaultOpen !== false };
    }

    toggle = () => this.setState(s => ({ aberto: !s.aberto }))

    render() {
        const { nodeKey, value, depth } = this.props;
        const { aberto } = this.state;
        const isObj = value !== null && typeof value === 'object';

        const keyEl = nodeKey !== null
            ? <><span style={styles.key}>"{nodeKey}"</span><span style={styles.colon}>: </span></>
            : null;

        if (!isObj) {
            let color = '#059669', display = `"${value}"`;
            if (typeof value === 'number')  { color = '#d97706'; display = String(value); }
            if (typeof value === 'boolean') { color = '#2563eb'; display = String(value); }
            if (value === null)             { color = '#9ca3af'; display = 'null'; }
            return (
                <div style={styles.node}>
                    {keyEl}
                    <span style={{ color }}>{display}</span>
                </div>
            );
        }

        const isArr = Array.isArray(value);
        const entries = isArr ? value.map((v, i) => [i, v]) : Object.entries(value);
        const [open, close] = isArr ? ['[', ']'] : ['{', '}'];
        const count = entries.length;

        return (
            <div style={styles.node}>
                <span onClick={this.toggle} style={styles.toggle}>
                    <span style={{ ...styles.arrow, transform: aberto ? 'rotate(90deg)' : 'rotate(0deg)' }}>▶</span>
                    {keyEl}
                    <span style={styles.bracket}>{open}</span>
                    {!aberto && (
                        <span style={styles.count}>
                            {count} {isArr ? 'item' : 'prop'}{count !== 1 ? 's' : ''}
                        </span>
                    )}
                </span>

                {aberto && (
                    <div style={styles.children}>
                        {entries.map(([k, v]) => (
                            <JsonNode
                                key={k}
                                nodeKey={isArr ? null : k}
                                value={v}
                                depth={(depth || 0) + 1}
                                defaultOpen={true}
                            />
                        ))}
                    </div>
                )}

                {aberto && <span style={styles.bracket}>{close}</span>}
                {!aberto && <span style={styles.bracket}>{close}</span>}
            </div>
        );
    }
}

export default class JsonViewer extends Component {

    parse = (valor) => {
        if (valor === null || valor === undefined) return null;
        if (typeof valor === 'object') return valor;
        try {
            return JSON.parse(valor);
        } catch {
            return valor;
        }
    }

    render() {
        const { dados, label, style } = this.props;

        const parsed = this.parse(dados);

        if (parsed === null || parsed === undefined) return null;

        return (
            <div style={{ ...styles.wrapper, ...style }}>
                {label && (
                    <label style={styles.label}>{label}</label>
                )}
                <div style={styles.box}>
                    <JsonNode value={parsed} nodeKey={null} depth={0} defaultOpen={true} />
                </div>
            </div>
        );
    }
}

const styles = {
    wrapper: {
        marginBottom: '12px',
    },
    label: {
        display: 'block',
        fontSize: '11px',
        textTransform: 'uppercase',
        letterSpacing: '0.5px',
        color: '#6c757d',
        marginBottom: '6px',
    },
    box: {
        background: '#f8f9fa',
        border: '1px solid #dee2e6',
        borderRadius: '6px',
        padding: '10px 14px',
        fontSize: '13px',
        fontFamily: 'monospace',
        lineHeight: 1.7,
        overflowX: 'auto',
    },
    node: {
        paddingTop: '1px',
        paddingBottom: '1px',
    },
    toggle: {
        cursor: 'pointer',
        userSelect: 'none',
        display: 'inline-flex',
        alignItems: 'center',
        gap: '4px',
    },
    arrow: {
        fontSize: '10px',
        color: '#6c757d',
        display: 'inline-block',
        transition: 'transform 0.15s',
        width: '10px',
    },
    key: {
        color: '#7c3aed',
        fontWeight: 500,
    },
    colon: {
        color: '#6c757d',
    },
    bracket: {
        color: '#6c757d',
    },
    count: {
        fontSize: '11px',
        color: '#6c757d',
        marginLeft: '4px',
    },
    children: {
        marginLeft: '16px',
        borderLeft: '1.5px solid #dee2e6',
        paddingLeft: '10px',
    },
};