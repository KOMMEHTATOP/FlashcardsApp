

const formatTotalHour = (time: String) => {
    const [h, m, s] = time.split(":").map(Number);
    return h + m / 60 + s / 3600;
};


export default formatTotalHour;