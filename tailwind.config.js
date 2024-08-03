module.exports = {
    mode: "jit",
    content: ["./**/*.razor", "./wwwroot/index.html"],
    theme: {
        extend: {
            colors: {
                slack_bg: "#2B092A",
                slack_nav: "#3E103F"
            }
        }
    },
    plugins: []
}; 