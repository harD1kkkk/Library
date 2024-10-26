import { UploadOutlined } from "@ant-design/icons";
import { Button, Upload } from "antd";
import React from "react";

const CoverImageUpload = ({ setFieldValue }) => {
  const handledUpload = ({ file }) => {
    setFieldValue("image", file);
  };

  return (
    <Upload
      beforeUpload={() => false}
      onChange={handledUpload}
      accept=".jpg, .jpeg, .png,"
    >
      <Button className="upload-button" icon={<UploadOutlined />}>
        Upload Cover Image
      </Button>
    </Upload>
  );
};

export default CoverImageUpload;
