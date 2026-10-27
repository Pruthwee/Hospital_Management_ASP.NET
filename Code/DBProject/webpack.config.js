/**
 * webpack.config.js
 *
 * Azure Static Web Apps - Webpack CSS Bundling Configuration
 * ----------------------------------------------------------
 * Remediates cr-css-1006: Multiple CSS Files Not Concatenated or HTTP/2
 *
 * This webpack configuration bundles all CSS/LESS/SCSS partials into single
 * concatenated output files for delivery via Azure CDN with HTTP/2 enabled.
 * The Azure Static Web Apps CI/CD pipeline runs `npm run build` which invokes
 * this webpack config to produce optimised, CDN-ready CSS bundles.
 *
 * Usage in Azure Static Web Apps workflow:
 *   app_build_command: "npm run build"
 *   output_location: "dist"
 */

const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');

module.exports = {
  mode: 'production',

  // Entry points: each key produces one concatenated CSS bundle
  entry: {
    // Font Awesome SCSS bundle (primary — preferred over LESS)
    'font-awesome': path.resolve(__dirname, 'assets/font-awesome/scss/font-awesome.scss'),

    // Application CSS bundle — concatenates all site-level CSS files
    'app': [
      path.resolve(__dirname, 'assets/css/bootstrap.min.css'),
      path.resolve(__dirname, 'assets/css/style.css'),
      path.resolve(__dirname, 'assets/css/demo.css'),
      path.resolve(__dirname, 'assets/css/form-elements.css'),
      path.resolve(__dirname, 'assets/css/grid-view.css'),
      path.resolve(__dirname, 'assets/css/patient-style.css'),
      path.resolve(__dirname, 'assets/css/footer-distributed-with-address-and-phones.css'),
    ],
  },

  output: {
    path: path.resolve(__dirname, 'dist'),
    // JS shim files produced by webpack for CSS-only entries (can be ignored)
    filename: 'js/[name].bundle.js',
    clean: true,
  },

  module: {
    rules: [
      // SCSS / SASS files
      {
        test: /\.s[ac]ss$/i,
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: 'css-loader',
            options: { sourceMap: false, importLoaders: 2 },
          },
          {
            loader: 'sass-loader',
            options: { sourceMap: false },
          },
        ],
      },
      // LESS files
      {
        test: /\.less$/i,
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: 'css-loader',
            options: { sourceMap: false, importLoaders: 1 },
          },
          {
            loader: 'less-loader',
            options: { sourceMap: false },
          },
        ],
      },
      // Plain CSS files
      {
        test: /\.css$/i,
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: 'css-loader',
            options: { sourceMap: false },
          },
        ],
      },
      // Font / image assets referenced inside CSS
      {
        test: /\.(woff|woff2|eot|ttf|otf|svg|png|jpg|gif)$/i,
        type: 'asset/resource',
        generator: {
          filename: 'assets/[name][ext]',
        },
      },
    ],
  },

  plugins: [
    new MiniCssExtractPlugin({
      // Output: dist/css/<entry-name>.bundle.css
      filename: 'css/[name].bundle.css',
    }),
  ],

  optimization: {
    minimizer: [
      // Minify and deduplicate CSS for Azure CDN delivery
      new CssMinimizerPlugin({
        minimizerOptions: {
          preset: ['default', { discardComments: { removeAll: false } }],
        },
      }),
    ],
  },

  // Suppress JS bundle size warnings for CSS-only entries
  performance: {
    hints: false,
  },
};
