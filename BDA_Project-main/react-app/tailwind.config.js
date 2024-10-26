module.exports = {
  content: ["./src/**/*.{js,jsx,ts,tsx}", "./src/**/*.{html,scss}"],
  theme: {
    extend: {
      fontFamily: {
        montserrat: ["Montserrat", "Arial", "sans-serif"],
        lato: ["Lato", "Arial", "sans-serif"],
        lora: ["Lora", "serif"],
        opensans: ["Open Sans", "sans-serif"],
        playfair: ["Playfair Display", "serif"],
        raleway: ["Raleway", "sans-serif"],
        robotoCondensed: ["Roboto Condensed", "sans-serif"],
        poppins: ["Poppins", "sans-serif"],
      },
      backgroundColor: {
        main: "blue-600",
        "light-gray": "#F7F7F7",
      },
      screens: {
        "x-sm": "514px",
      },
    },
  },
  plugins: [],
};
