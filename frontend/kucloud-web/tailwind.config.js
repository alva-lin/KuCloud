const {
  iconsPlugin,
  dynamicIconsPlugin,
  getIconCollections,
} = require('@egoist/tailwindcss-icons');

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './app/**/*.{js,ts,jsx,tsx,mdx}',
    './components/**/*.{js,ts,jsx,tsx,mdx}'
  ],
  theme: {
    extend: {},
  },
  plugins: [
    iconsPlugin({
      collections: getIconCollections([ 'fluent' ]),
    }),
    dynamicIconsPlugin(),
  ],
};
