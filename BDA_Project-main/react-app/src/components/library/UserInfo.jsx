import { UserOutlined, MailOutlined } from "@ant-design/icons";
import { Button, Modal, Row, Col, Typography, message } from "antd";
import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { selectUser, signOut } from "../../store/slices/userSlice";

const { Title, Text } = Typography;

const InfoRow = ({ icon, label, value }) => (
  <Row gutter={[16, 16]} justify="start" align="middle">
    <Col span={4}>{icon}</Col>
    <Col span={16}>
      <Text strong>{label}:</Text>{" "}
      <Text className="text-lg text-gray-700">{value}</Text>
    </Col>
  </Row>
);

const UserInfo = () => {
  const dispatch = useDispatch();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const user = useSelector(selectUser);

  const showModal = () => setIsModalOpen(true);
  const handleOk = () => setIsModalOpen(false);
  const handleCancel = () => setIsModalOpen(false);

  const userInfo = [
    {
      icon: <UserOutlined style={{ fontSize: "20px", color: "#595959" }} />,
      label: "Full Name",
      value: user.name,
    },
    {
      icon: <MailOutlined style={{ fontSize: "20px", color: "#595959" }} />,
      label: "Email",
      value: user.email,
    },
  ];

  const logoutUser = () => {
    message.info(`You have logged out, ${user.name}`)
    dispatch(signOut());
  };

  return (
    <>
      <Button
        type="primary"
        shape="round"
        onClick={showModal}
        icon={<UserOutlined />}
        size="large"
        className="shadow-md hover:shadow-lg ml-6"
      >
        My Profile
      </Button>

      <Modal
        title={
          <Title level={3} className="text-center mb-0">
            User Information
          </Title>
        }
        open={isModalOpen}
        onOk={handleOk}
        onCancel={handleCancel}
        footer={[
          <Button
            key="logout"
            type="danger"
            onClick={() => {
              logoutUser();
              handleOk();
            }}
            className="bg-red-500 text-white hover:bg-red-600 hover:text-white transition-all"
          >
            Logout
          </Button>,
          <Button key="close" type="primary" onClick={handleOk}>
            Close
          </Button>,
        ]}
        centered
        className="custom-modal p-4 rounded-lg"
      >
        <Row gutter={[16, 16]} justify="center" align="middle">
          <Col span={24} className="text-center">
            <UserOutlined style={{ fontSize: "60px", color: "#1890ff" }} />
          </Col>
          <Col span={24}>
            {userInfo.map((info) => (
              <InfoRow
                key={info.label}
                icon={info.icon}
                label={info.label}
                value={info.value}
              />
            ))}
          </Col>
        </Row>
      </Modal>
    </>
  );
};

export default UserInfo;
