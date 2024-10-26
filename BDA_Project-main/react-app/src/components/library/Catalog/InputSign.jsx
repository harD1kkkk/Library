import "../../../styles/pages/Sign.scss";

const InputSign = ({ label, type, name, value, onChange, placeholder }) => {
  return (
    <div className="mt-8">
      <label className="text-gray-800 text-xs block mb-2">{label}</label>
      <input
        name={name}
        type={type}
        value={value}
        onChange={onChange}
        required
        className="sign-input-field"
        placeholder={placeholder}
      />
    </div>
  );
};

export default InputSign;
