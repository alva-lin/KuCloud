module.exports = {
  extends: [
    'plugin:prettier/recommended',
    'plugin:@next/next/recommended',
    'plugin:jest/recommended',
    'mantine',
  ],
  plugins: [
    'prettier',
    'testing-library',
    'jest',
  ],
  overrides: [
    {
      files: [ '**/?(*.)+(spec|test).[jt]s?(x)' ],
      extends: [ 'plugin:testing-library/react' ],
    },
  ],
  parserOptions: {
    project: './tsconfig.json',
  },
  rules: {
    'prettier/prettier': 'error',
    'react/react-in-jsx-scope': 'off',
    'import/extensions': 'off',
  },
};
