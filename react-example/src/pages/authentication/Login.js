import React from 'react'
import { Row, Col, Form, Input, Button } from 'antd';
import logo from '../../images/logo.png'
import { connect } from 'react-redux'
import { login } from '../../redux/actions/actions'
import { Link } from 'react-router-dom';

class Login extends React.Component {

  formRef = React.createRef();

  handleSubmit = async (values) => {
    const formData = new FormData();
    formData.append('login_username', values.username);
    formData.append('login_password', values.password);

    try {
      const response = await fetch('http://localhost:8080/Cors/demo', {
        method: 'POST',
        body: formData,
        credentials: 'include'
      })
      alert(await response.text());
    } catch (e) {
      console.error(e);
    }
  }

  render() {
    return (
      <div>
        <Row justify="center" align="middle" style={{ height: '100vh' }}>
          <Col md={8} sm={12} xs={24}>
            <img 
              src={logo}
              alt="Logo Fakebook"
              style={{ width: '100%', paddingLeft: '24px', paddingRight: '24px', maxWidth: '400px' }}
            />
          </Col>

          <Col md={8} sm={12} xs={24}>
            <Form
              ref={this.formRef}
              onFinish={this.handleSubmit}
              className="login-form"
              style={{ maxWidth: '400px', width: '100%' }}
              layout="vertical"
            >

              <Form.Item
                label="Username"
                name="username"
                rules={[{ required: true, message: 'Please input your nickname!' }]}
              >
                <Input />
              </Form.Item>

              <Form.Item
                label="Password"
                name="password"
                rules={[{ required: true, message: 'Please input your password!' }]}
              >
                <Input.Password />
              </Form.Item>

              <Row>
                <Col span={12}>
                  <Form.Item>
                    <Link to='/signup'>
                      <Button block type="link">
                        Signup
                      </Button>
                    </Link>
                  </Form.Item>
                </Col>

                <Col span={12}>
                  <Form.Item>
                    <Button block type="primary" htmlType="submit">
                      Log in
                    </Button>
                  </Form.Item>
                </Col>
              </Row>

            </Form>
          </Col>
        </Row>
      </div>
    )
  }
}

const mapDispatchToProps = {
  login: login
}

export default connect(null, mapDispatchToProps)(Login);
