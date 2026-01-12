import type { Appearance } from '@stripe/stripe-js';

export const getStripeAppearance = (darkMode: boolean): Appearance => ({
  theme: darkMode ? 'night' : 'stripe',
  variables: {
    colorPrimary: '#c4948a',
    colorBackground: darkMode ? '#1a1414' : '#ffffff',
    colorText: darkMode ? '#f2e7e7' : '#140f0e',
    colorDanger: '#a11132',
    fontFamily: 'Manrope, sans-serif',
    borderRadius: '8px'
  },
  rules: {
    '.Input': {
      backgroundColor: darkMode ? '#241c1c' : '#fff4ef',
      borderColor: '#c4948a'
    },
    '.Label': {
      color: darkMode ? '#c5b1b1' : '#643c34'
    }
  }
});
