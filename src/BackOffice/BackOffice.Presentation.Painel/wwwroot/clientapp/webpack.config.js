const path = require("path");
var glob = require("glob");
const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
    entry: toObject(glob.sync("./src/views/**/*.js")),
    cache: {
        type: 'filesystem',
    },
    output: {
        path: path.resolve(__dirname, "./dist/views"),
        filename: "[name].js",
        publicPath: "/dist/",
        pathinfo: false
    },
    optimization: {
        removeAvailableModules: false,
        removeEmptyChunks: false,
        splitChunks: false,
        minimizer: [
            new TerserPlugin({ extractComments: false })
        ],
    },
    devServer: {
        devMiddleware: {
            writeToDisk: (filePath) => {
                return !/hot-update/i.test(filePath);
            },
        },
    },
    module: {
        rules: [
            {
                use: {
                    loader: "babel-loader",
                    options: {
                        cacheCompression: false,
                        cacheDirectory: true,
                    },
                },
                test: /\.js$/,
                include: path.resolve(__dirname, 'src'),
                exclude: /node_modules/ //excludes node_modules folder from being transpiled by babel. We do this because it's a waste of resources to do so.
            },

            
            {
                test: /\.(png|jp(e*)g|svg|gif|mp3)$/,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            name: "[folder]/[name].[ext]",
                            cacheCompression: false,
                            cacheDirectory: true,
                        },
                    },
                ],
            },

            {
                test: /\.css$/i,
                use: ['style-loader', 'css-loader'],
            },
        ],

    },
};

function toObject(paths) {
    var ret = {};

    paths.forEach(function (pathItem) {
        // you can define entry names mapped to [name] here
        let resolveBarras = path.normalize("./src/views/");
        ret[pathItem.replace(resolveBarras, "").replace(".js", "")] = __dirname + "/" + pathItem;
    });

    
    /*for (var key in ret) {
        if (ret.hasOwnProperty(key)) {
            console.log("Chave: " + key + ", Valor: " + ret[key]);
        }
    }*/

    return ret;
}

 